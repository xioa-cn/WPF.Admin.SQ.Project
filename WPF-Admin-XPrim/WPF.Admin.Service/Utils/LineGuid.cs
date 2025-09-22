namespace WPF.Admin.Service.Utils
{
    public static class LineGuid
    {
        /// <summary>
        /// 生成22位的唯一数字，并发可用
        /// </summary>
        /// <returns></returns>
        public static string UniqueDTO22()
        {
            System.Threading.Thread.Sleep(1);
            Random rd = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(),0));
            string strUniqueId = DateTime.Now.ToString("yyyyMMddHHmmssffff")+rd.Next(1000,9999);
            return strUniqueId;
        }
    }
}