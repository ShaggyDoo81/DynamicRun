namespace shg.dynRunner.Application.Models
{
    public class DynCodeData
    {
        public string Identifier { get; set; }
        public string Code { get; set; }    
        public DateTime UpdatedTime { get; set; }
        public List<Type> AdditionalTypes { get; set; } = new();

        public DynCodeData(string identifier, string code, DateTime updatedTime, List<Type>? additionalTypes)
        {
            Identifier = identifier;
            Code = code;
            UpdatedTime = updatedTime;
            AdditionalTypes = additionalTypes ?? new();
        }
    }
}
