namespace FclEx.Fw.Domain.Entities.Auditing
{
    public interface ICreationAudited<TUserId> : IHasCreationTime
        where TUserId : struct
    {
        TUserId? CreatorUserId { get; set; }
    }

    public interface ICreationAudited : ICreationAudited<int>
    {
    }
}