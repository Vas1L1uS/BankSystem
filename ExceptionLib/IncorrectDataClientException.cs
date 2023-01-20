using System;

namespace ExceptionLib
{
    public class IncorrectDataClientException : Exception
    {
        public IncorrectDataClientException (string Msg) : base()
        {
            Console.WriteLine(Msg);
        }

        public IncorrectDataClientException(string Msg, uint codeError) : base()
        {
            CodeError = codeError;
            Console.WriteLine(Msg);
        }

        public uint CodeError { get; }
    }
}
