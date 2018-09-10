namespace FclEx.Fw.Domain.Entities.Auditing
{
    public interface IModificationAudited<TUserId> : IHasModificationTime
        where TUserId : struct
    {
        TUserId? LastModifierUserId { get; set; }
    }

    public interface IModificationAudited : IModificationAudited<int>
    {
    }
}