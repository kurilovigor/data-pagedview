using System.Reflection;

namespace Kurilov.Data.PagedView
{
    internal class Key
    {
        public SortDirection SortDirection { get; set; }

        public MemberInfo Member {  get; set; }

        public Key(MemberInfo member, SortDirection sortDirection = SortDirection.Ascending)
        { 
            this.Member = member;    
            this.SortDirection = sortDirection;
        }
    }
}
