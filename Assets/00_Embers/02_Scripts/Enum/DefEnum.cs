namespace NOLDA
{
    /// <summary>
    /// 캐릭터
    /// </summary>
    public enum Class
    {
        NONE = -1,
        WARRIOR = 0,
        MAGE = 1,
        ROGUE,
    }

    public enum Gender
    {
        MALE = 0,
        FEMALE = 1,
    }
    //
    
    /// <summary>
    /// DBManager
    /// </summary>
    public enum SignUpResult
    {
        SUCCESS,
        DUPLICATE,
        ERROR
    }

    public enum LoginResult
    {
        SUCCESS,
        PWWRONG,
        IDWRONG,
        ERROR
    }

    public enum CreateCharacterResult
    {
        SUCCESS,
        DUPLICATE,
        ERROR
    }
    //
    
    /// <summary>
    /// ChunkState
    /// </summary>
    public enum ChunkLoadState
    {
        NONE,       // 로드되지 않음
        LOADING,    // 로딩 중
        LOADED,     // 로드 완료
        UNLOADING   // 언로드 중
    }
    
}