namespace RoboUI.Filters
{
    public interface IFilterNode
    {
        void Accept(IFilterNodeVisitor visitor);
    }
}
