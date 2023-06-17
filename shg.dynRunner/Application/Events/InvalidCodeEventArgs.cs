namespace shg.dynRunner.Application.Events
{
    public class InvalidCodeEventArgs :  EventArgs
    {
        public string Code { get; set; }
        public string Identifier { get; set; }
        public List<string> Errors { get; set; }

        public InvalidCodeEventArgs(string code, string identifier, List<string> errors)
        {
            Code = code;
            Identifier = identifier;
            Errors = errors;
        }   
    }
}
