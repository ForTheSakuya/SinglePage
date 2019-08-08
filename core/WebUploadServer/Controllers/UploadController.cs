
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebUploadServer.Bll;
using WebUploadServer.Models;

namespace WebUploadServer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IOptions<Custom> _configuration;
        public UploadController(IOptions<Custom> customSettings)
        {
            _configuration = customSettings;
        }

        private UploadBll _uploader;
        private UploadBll Uploader => _uploader ??
                                      (_uploader = new UploadBll(_configuration));

        /// <summary>
        /// 分片上传
        /// 上传
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("multipart")]
        public async Task<ActionResult<UploadTask>> Upload([FromForm]UploadTask dto)
        {
            var result = await Uploader.MultipartUpload(dto);
            return result;
        }

        public ActionResult<string> Upolad()
        {
            return JsonConvert.SerializeObject(_configuration);
        }

        [HttpPost, Route("")]
        public ActionResult<UploadTask> Test([FromForm]UploadTask dto)
        {
            return dto;
        }
    }
}
