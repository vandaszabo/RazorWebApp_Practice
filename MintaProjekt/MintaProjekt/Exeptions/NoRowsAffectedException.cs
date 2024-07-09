namespace MintaProjekt.Exeptions
{
    public class NoRowsAffectedException : Exception
    {
        public NoRowsAffectedException() : base() { }

        public NoRowsAffectedException(string message) : base(message) { }

        public NoRowsAffectedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
