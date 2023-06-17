namespace shg.dynRunner.Application.Models
{
    public class DynCodeData
    {
        public string Identifier { get; set; }
        public string Code { get; set; }
        public string ClassName { get; set; }
        public DateTime UpdatedTime { get; set; }
        public List<Type> AdditionalTypes { get; set; } = new();

        public DynCodeData(string identifier, 
            string code, 
            string className, 
            DateTime updatedTime, 
            List<Type>? additionalTypes)
        {
            Identifier = identifier;
            Code = code;
            ClassName = className;
            UpdatedTime = updatedTime;
            AdditionalTypes = additionalTypes ?? new();
        }
    }
}
