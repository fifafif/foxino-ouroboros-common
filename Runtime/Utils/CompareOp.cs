namespace Ouroboros.Common.Utils
{
    public enum CompareOp
    {
        Less = 0,
        More = 1,
        Equal = 2,
        LessOrEqual = 3,
        MoreOrEqual = 4
    }

    public static class CompareOpExtensions
    {
        public static bool IsTrue(this CompareOp compareOp, float left, float right)
        {
            switch (compareOp)
            {
                case CompareOp.Less:
                    return left < right;

                case CompareOp.More:
                    return left > right;

                case CompareOp.Equal:
                    return left == right;

                case CompareOp.LessOrEqual:
                    return left <= right;

                case CompareOp.MoreOrEqual:
                    return left >= right;

                default:
                    return false;
            }
        }
    }
}