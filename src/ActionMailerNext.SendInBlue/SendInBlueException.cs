namespace ActionMailerNext.SendInBlue
{
    using System;

    [Serializable]
    public class SendInBlueException : Exception
    {
        public SendInBlueException() { }

        public SendInBlueException(string message)
            : base(message) { }

        public SendInBlueException(string message, Exception inner)
            : base(message, inner) { }
    }
}
