namespace SuperScrabble.Common.Exceptions
{
    public abstract class ValidationFailedException : Exception
    {
        public string ErrorCode
        {
            get
            {
                string name = this.GetType().Name;
                int lastIndex = name.LastIndexOf("Exception");
                return name.Remove(lastIndex);
            }
        }
    }
}
