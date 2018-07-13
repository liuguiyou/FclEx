namespace FclEx.Http.Core
{
    public struct HttpFileInfo
    {
        public HttpFileInfo(string name, string fileName, string contentType)
        {
            Name = name;
            FileName = fileName;
            ContentType = contentType;
        }

        public string Name { get; }
        public string FileName { get; }
        public string ContentType { get; }
    }
}