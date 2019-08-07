
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebUploadServer.Bll;
using WebUploadServer.Models;

namespace WebUploadServer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private UploadBll _uploader;
        private UploadBll Uploader => _uploader ?? (_uploader = new UploadBll());

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

        public ActionResult Upolad()
        {
            return NoContent();
        }

        [HttpPost, Route("")]
        public ActionResult<UploadTask> Test([FromForm]UploadTask dto)
        {
            return dto;
        }
    }
}
