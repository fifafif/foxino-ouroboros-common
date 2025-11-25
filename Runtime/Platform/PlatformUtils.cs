namespace Ouroboros.Common.Platform
{
    public static class PlatformUtils
    {
        public static bool IsMobile()
        {
#if OUROBOROS_MOBILE
            
            return true;

#else

            return false;

#endif
        }
    }
}
