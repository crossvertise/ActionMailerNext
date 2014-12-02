namespace ActionMailerNext.Implementations.Mandrill
{
    /*the sending status of the recipient - either "sent", "queued", "scheduled", "rejected", or "invalid" */
    public enum MandrillStatus
    {
        SENT,
        QUEUED,
        SCHEDULED,
        REJECTED,
        INVALID
    }
}