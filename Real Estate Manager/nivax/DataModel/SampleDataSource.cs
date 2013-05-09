using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace BricksStyle.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : BricksStyle.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

       

        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var group1 = new SampleDataGroup("Group-1",
                 "Executions",
                 "Executions",
                 "Assets/10.jpg",
                 "Real estate is Property consisting of land and the buildings on it, along with its natural resources such as crops, minerals, or water; immovable property of this nature");

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item1",
                 "Introduction",
                 "Introduction",
                 "Assets/11.jpg",
                 "Real estate is Property consisting of land and the buildings on it, along with its natural resources such as crops, minerals, or water; immovable property of this nature",
                 "\n\n\n\n\n\n\n\n\nReal estate business in Mexico, Canada, Guam, and Central America operates differently than in the United States. Some similarities include legal formalities (with professionals such as real estate agents generally employed to assist the buyer); taxes need to be paid (but typically less than those in U.S.); legal paperwork will ensure title; and a neutral party such as a title company will handle documentation and money to make the smooth exchange between the parties. Increasingly, U.S. title companies are doing work for U.S. buyers in Mexico and Central America.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "Terminology",
                 "Terminology",
                 "Assets/12.jpg",
                 "In Thailand it is possible for a foreigner to own a condominium freehold provided ownership does not exceed 49% of the total building; it is not easily possible for a foreigner to own land but normal practice is that property can be purchased then Land acquired under a 30 year lease option",
                 "\n\n\n\n\n\n\n\n\nIn Thailand it is possible for a foreigner to own a condominium freehold provided ownership does not exceed 49% of the total building; it is not easily possible for a foreigner to own land but normal practice is that property can be purchased then Land acquired under a 30 year lease option; Until recently it was considered by most legal advisors that the ownership of land by a foreigner through a Thai Limited Company was acceptable, although the Law clearly states that foreigners cannot own land in Thailand. The Government has now made clear that such ownership may be illegal. The legitimacy of such ownership depends on the status of the Thai Shareholders who must be shown to be active and financially participating shareholders.",
                 53,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item3",
                 "Residential real estate",
                 "Residential real estate",
                 "Assets/13.jpg",
                 "The legal arrangement for the right to occupy a dwelling in some countries is known as the housing tenure. Types of housing tenure include owner occupancy, Tenancy, housing cooperative, condominiums (individually parceled properties in a single building).",
                 "\n\n\n\n\n\n\n\n\nThe legal arrangement for the right to occupy a dwelling in some countries is known as the housing tenure. Types of housing tenure include owner occupancy, Tenancy, housing cooperative, condominiums (individually parceled properties in a single building), public housing, squatting, and cohousing. The occupants of a residence constitute a household. Residences can be classified by, if, and how they are connected to neighboring residences and land. Different types of housing tenure can be used for the same physical type. For example, connected residents might be owned by a single entity and leased out, or owned separately with an agreement covering the relationship between units and common areas and concerns.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "Property management",
                 "Property management",
                 "Assets/14.jpg",
                 "Property management is the operation, control, and oversight of real estate as used in its most broad terms. Management indicates a need to be cared for, monitored and accountability given for its useful life and condition. This is much akin to the role of management in any business.",
                 "\n\n\n\n\n\n\n\n\nProperty management is the operation, control, and oversight of real estate as used in its most broad terms. Management indicates a need to be cared for, monitored and accountability given for its useful life and condition. This is much akin to the role of management in any business. Property management is also the management of personal property, equipment, tooling and physical capital assets that are acquired and used to build, repair and maintain end item deliverables. Property management involves the processes, systems and manpower required to manage the life cycle of all acquired property as defined above including acquisition, control, accountability, responsibility, maintenance, utilization and disposition.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item5",
                 "Corporate Real Estate",
                 "Corporate Real Estate",
                 "Assets/15.jpg",
                 "Corporate real estate is the real property held or used by a business enterprise or organization for its own operational purposes. A corporate real estate portfolio typically includes a corporate headquarters and a number of branch offices, and perhaps also various manufacturing and retail sites.",
                 "\n\n\n\n\n\n\n\n\nCorporate real estate may also describe the functional practice, department, or profession that is concerned with the planning, acquisition, management, and administration of real property on behalf of a company. Generally, Corporate real estate professionals approach the real estate market from the buy-side, or demand perspective, similar to corporate purchasing or procurement. As such, they seek to contain costs, and may benefit from economic environments that are described by most as weak. Although closely related to facilities management and property management, Corporate real estate as a concept is usually broader in corporate functional scope but more narrow within the real estate sector. For instance, Corporate Real Estate professionals (or departments) typically dedicate greater emphasis and time on multi-site long-range planning (often called portfolio planning or strategic planning). However, Corporate real estate is almost exclusively focused on commercial properties types (mostly office, with industrial and retail depending on the company); residential properties are rare in a corporate portfolio.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item6",
                 "Cohousing",
                 "Cohousing",
                 "Assets/16.jpg",
                 "A cohousing community is a type of intentional community composed of private homes supplemented by shared facilities.",
                 "\n\n\n\n\n\n\n\n\n\n\n\n\nA cohousing community is a type of intentional community composed of private homes supplemented by shared facilities. The community is planned, owned and managed by the residents – who also share activities which may include cooking, dining, child care, gardening, and governance of the community. Common facilities may include a kitchen, dining room, laundry, child care facilities, offices, internet access, guest rooms, and recreational features.",
                 53,
                 49,
                 group1));
            
            this.AllGroups.Add(group1);

             var group2 = new SampleDataGroup("Group-2",
                 "Directives",
                 "Directives",
                 "Assets/20.jpg",
                 "Housing tenure refers to the financial arrangements under which someone has the right to live in a house or apartment. The most frequent forms are tenancy, in which rent is paid to a landlord, and owner occupancy.");

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                 "Housing tenure",
                 "Housing tenure",
                 "Assets/21.jpg",
                 "Housing tenure refers to the financial arrangements under which someone has the right to live in a house or apartment. The most frequent forms are tenancy, in which rent is paid to a landlord, and owner occupancy.",
                 "\n\n\n\n\n\n\n\n\nHousing tenure refers to the financial arrangements under which someone has the right to live in a house or apartment. The most frequent forms are tenancy, in which rent is paid to a landlord, and owner occupancy. Mixed forms of tenure are also possible. The basic forms of tenure can be subdivided, for example an owner-occupier may own a house outright, or it may be mortgaged. In the case of tenancy, the landlord may be a private individual, a non-profit organization such as a housing association, or a government body, as in public housing. Surveys used in social science research frequently include questions about housing tenure, because it is a useful proxy for income or wealth, and people are less reluctant to give information about it.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item2",
                 "Owner-occupier",
                 "Owner-occupier",
                 "Assets/22.jpg",
				 "An owner-occupier (also known as an owner-occupant or home owner) is a person who lives in and owns the same home.",
                 "\n\n\n\n\n\n\n\n\nSome homes are constructed by the owners with the intent to occupy. Many are inherited. A large number are purchased, as new homes from a real-estate developer or as an existing home from a previous landlord or owner-occupier. A house is usually the most expensive single purchase an individual or family makes, and often costs several times the annual household income. Given the high cost, most individuals do not have enough savings on hand to pay the entire amount outright. In developed countries, mortgage loans are available from financial institutions in return for interest. If the home owner fails to meet the agreed repayment schedule, a foreclosure (known as a repossession in some countries) may result.",
                 53,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item3",
                 "Leasehold estate",
                 "Leasehold estate",
                 "Assets/23.jpg",
                 "A leasehold estate is an ownership of a temporary right to hold land or property in which a lessee or a tenant holds rights of real property by some form of title from a lessor or landlord.",
                 "\n\n\n\n\n\n\n\n\nA leasehold estate is an ownership of a temporary right to hold land or property in which a lessee or a tenant holds rights of real property by some form of title from a lessor or landlord. Although a tenant does hold rights to real property, a leasehold estate is typically considered personal property. Leasehold is a form of land tenure or property tenure where one party buys the right to occupy land or a building for a given length of time. As lease is a legal estate, leasehold estate can be bought and sold on the open market. A leasehold thus differs from a freehold or fee simple where the ownership of a property is purchased outright and thereafter held for an indeterminate length of time, and also differs from a tenancy where a property is let (rented) on a periodic basis such as weekly or monthly.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item4",
                 "Housing cooperative",
                 "Housing cooperative",
                 "Assets/24.jpg",
                 "A housing cooperative is a legal entity, usually a corporation, renting own real estate, consisting of one or more residential buildings; it is one type of housing tenure.",
                 "\n\n\n\n\n\n\n\n\nA housing cooperative is a legal entity, usually a corporation, renting own real estate, consisting of one or more residential buildings; it is one type of housing tenure. Housing cooperatives are a distinctive form of home ownership that has many characteristics that make it different than other residential arrangements such as single family ownership, condominiums and renting. The corporation is membership based, with membership granted by way of a share purchase in the cooperative. Each shareholder in the legal entity is granted the right to occupy one housing unit. A primary advantage of the housing cooperative is the pooling of the members’ resources so that their buying power is leveraged, thus lowering the cost per member in all the services and products associated with home ownership.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item5",
                 "Public housing",
                 "Public housing",
                 "Assets/25.jpg",
                 "Public housing is a form of housing tenure in which the property is owned by a government authority, which may be central or local.",
                 "\n\n\n\n\n\n\n\n\nPublic housing is a form of housing tenure in which the property is owned by a government authority, which may be central or local. Social housing is an umbrella term referring to rental housing which may be owned and managed by the state, by non-profit organizations, or by a combination of the two, usually with the aim of providing affordable housing. Social housing can also be seen as a potential remedy to housing inequality. Although the common goal of public housing is to provide affordable housing, the details, terminology, definitions of poverty and other criteria for allocation vary within different contexts.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item6",
                 "Squatting",
                 "Squatting",
                 "Assets/26.jpg",
                 "Squatting consists of occupying an abandoned or unoccupied area of land and/or a building – usually residential – that the squatter does not own, rent or otherwise have lawful permission to use.",
                 "\n\n\n\n\n\n\n\n\nSquatting consists of occupying an abandoned or unoccupied area of land and/or a building – usually residential –[1] that the squatter does not own, rent or otherwise have lawful permission to use. \n\nAuthor Robert Neuwirth suggests that there are one billion squatters globally, that is, about one in every seven people on the planet. Yet, according to Kesia Reeve, squatting is largely absent from policy and academic debate and is rarely conceptualized, as a problem, as a symptom, or as a social or housing movement.",
                 53,
                 49,
                 group2));
            
            this.AllGroups.Add(group2);
			
           
        }
    }
}
