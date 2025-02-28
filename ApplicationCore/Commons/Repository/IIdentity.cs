namespace ApplicationCore.Commons.Repository;

public interface IIdentity<TK>: IComparable<TK> where TK: IComparable<TK>
{
    public TK Id
    {
        get;
        set;
    }

    int IComparable<TK>.CompareTo(TK? other)
    {
        return CompareTo(other);
    }
}