namespace WPF.Admin.Themes.I18n
{
    public static class CSharpI18n
    {
        /// <summary>
        /// 使用I18n
        /// </summary>
        /// <returns></returns>
        public static (Func<string, string> t, string usingLang) UseI18n()
        {
            return (I18nManager.Instance.GetString, I18nManager.Instance.UsingLanguage);
        }
    }
}