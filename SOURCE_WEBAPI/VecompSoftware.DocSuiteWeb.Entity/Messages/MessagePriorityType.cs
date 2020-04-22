namespace VecompSoftware.DocSuiteWeb.Entity.Messages
{
    public class MessagePriorityType
    {
        private MessagePriorityType() { }

        /// <summary>
        /// The email has normal priority.
        /// </summary>

        public const string Normal = "0";

        /// <summary>
        /// The email has low priority.
        /// </summary>
        public const string Low = "1";

        /// <summary>
        /// The email has high priority.
        /// </summary>
        public const string High = "2";
    }
}
