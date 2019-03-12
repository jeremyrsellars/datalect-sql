namespace Datalect.Sql
{
    public class BoundValue
    {
        public static BoundValue Create(object value) => new BoundValue(value, null);
        public static BoundValue CreateDescribed(string description, object value) => new BoundValue(value, description);
        public object Value { get; }
        public string Description { get; }
        public BoundValue(object value, string optionalDescription)
        {
            Description = optionalDescription;
            Value = value;
        }
    }
}
