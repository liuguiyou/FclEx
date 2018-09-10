namespace FclEx.Fw.Domain.Entities.Auditing
{
    public interface IDeletionAudited<TUserId> : IHasDeletionTime
        where TUserId : struct
    {
        TUserId? DeleterUserId { get; set; }
    }

    public interface IDeletionAudited : IDeletionAudited<int>
    {
    }
}