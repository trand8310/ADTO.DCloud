namespace ADTO.DCloud.Authentication.External.Weixin;

public class Jscode2SessionResult
{
    public string openid { get; set; }
    public string session_key { get; set; }

    /// <summary>
    /// 如果开发者帐号下存在同主体的公众号，并且该用户已经关注了该公众号。开发者可以直接通过 wx.login + code2Session 获取到该用户 UnionID，无须用户再次授权。
    /// </summary>
    public string unionid { get; set; }

    public int errcode { get; set; }

    public string errmsg { get; set; }
}
