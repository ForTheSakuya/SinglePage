using System.ServiceModel;
using System.Threading.Tasks;

namespace WebUploadServer.BLL.Common
{
    public class WebService
    {
        private readonly string _webServiceAdress;

        public WebService(string webServiceAdress)
        {
            _webServiceAdress = webServiceAdress;
        }
        /*
         * 编辑项目文件添加<DotNetCliToolReference Include="dotnet-svcutil" Version="1.0.*" />
         * CMD使用 dotnet restore 生成dotnet-svcutil
         * CMD使用如 dotnet http://172.16.160.42:8080/SobeyDCMP/services/XMSImport?wsdl 下载引用类
         */
        public async Task<importServiceResponse> Import(string xml)
        {
            // 创建 HTTP 绑定对象
            var binding = new BasicHttpBinding();
            // 根据 WebService 的 URL 构建终端点对象
            var endpoint = new EndpointAddress($"{_webServiceAdress}/SobeyDCMP/services/XMSImport");
            // 创建调用接口的工厂，注意这里泛型只能传入接口
            var factory = new ChannelFactory<ImportServiceTemplateChannel>(binding, endpoint);
            // 从工厂获取具体的调用实例
            var callClient = factory.CreateChannel();
            // 调用具体的方法，这里是 sfexpressServiceAsync 方法。
            var res = await callClient.importServiceAsync(new importService
            {
                Body = new importServiceBody
                {
                    source = xml
                }
            });
            return res;
        }
    }
}
