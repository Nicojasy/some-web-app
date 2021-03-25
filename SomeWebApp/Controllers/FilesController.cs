using SomeWebApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SomeWebApp.Infrastructure.File;
using SomeWebApp.Core.Entities;

namespace SomeWebApp.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly long _fileSizeLimit;
        private readonly ILogger<FilesController> _logger;
        private readonly string[] _permittedExtensions = { ".png", ".jpg", ".bmp" };
        private readonly string _targetFilePath;

        // Get the default form options so that we can use them to set the default 
        // limits for request body data.
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public FilesController(ILogger<FilesController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _fileSizeLimit = configuration.GetValue<long>("FileServiceSettings:FileSizeLimit");
            _unitOfWork = unitOfWork;

            // To save physical files to a path provided by configuration:
            _targetFilePath = configuration.GetValue<string>("FileServiceSettings:StoredFilesPath");

            // To save physical files to the temporary files folder, use:
            //_targetFilePath = Path.GetTempPath();
        }

        #region snippet_UploadPhysical
        [HttpPost, Route("upload")]
        [DisableFormValueModelBinding]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhysical()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (Error 1).");
                // TODO: Log error

                return BadRequest(ModelState);
            }

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    // This check assumes that there's a file
                    // present without form data. If form data
                    // is present, this method immediately fails
                    // and returns the model error.
                    if (!MultipartRequestHelper
                        .HasFileContentDisposition(contentDisposition))
                    {
                        ModelState.AddModelError("File",
                            $"The request couldn't be processed (Error 2).");
                        // TODO: Log error

                        return BadRequest(ModelState);
                    }
                    else
                    {
                        // Don't trust the file name sent by the client. To display
                        // the file name, HTML-encode the value.
                        var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                                contentDisposition.FileName.Value);
                        var trustedFileNameForFileStorage = Path.GetRandomFileName();

                        // **WARNING!**
                        // In the following example, the file is saved without
                        // scanning the file's contents. In most production
                        // scenarios, an anti-virus/anti-malware scanner API
                        // is used on the file before making the file available
                        // for download or for use by other systems. 
                        // For more information, see the topic that accompanies 
                        // this sample.



                        /*
                        // TODO: check virus
                        using (var httpClient = new HttpClient())
                        {
                            httpClient.DefaultRequestHeaders
                                .Accept
                                .Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

                            using (var response = await httpClient.PostAsync("https://www.virustotal.com/api/v3/files"))
                            {
                                string apiResponse = await response.Content.ReadAsStringAsync();

                                try
                                {
                                    reservationList = JsonSerializer.Deserialize<List<Reservation>>(apiResponse, new JsonSerializerOptions
                                    {
                                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                    });
                                }
                                catch
                                {
                                    return BadRequest(apiResponse);
                                }
                            }
                        }
                        @"curl --request POST \
                        --url https://www.virustotal.com/api/v3/files \
                        --header 'x-apikey: <your API key>' \
                        --form file=@/path/to/file"
                        */


                        /*
                        // TODO: check verif
                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                            section, contentDisposition, ModelState,
                            _permittedExtensions, _fileSizeLimit);

                        // process each image
                        const int chunkSize = 1024;
                        var buffer = new byte[chunkSize];
                        var bytesRead = 0;
                        var fileName = GetFileName(section.ContentDisposition);

                        using (var stream = new FileStream(fileName, FileMode.Append))
                        {
                            do
                            {
                                bytesRead = await section.Body.ReadAsync(buffer, 0, buffer.Length);
                                stream.Write(buffer, 0, bytesRead);

                            } while (bytesRead > 0);
                        }

                         */



                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }

                        const int chunkSize = 1024;
                        var buffer = new byte[chunkSize];
                        var bytesRead = 0;

                        // TODO: add additional path for filepath
                        using (var targetStream = System.IO.File.Create(
                            Path.Combine(_targetFilePath, trustedFileNameForFileStorage)))
                        {
                                do
                                {
                                    bytesRead = await section.Body.ReadAsync(buffer.AsMemory(0, buffer.Length));
                                    targetStream.Write(buffer, 0, bytesRead);

                                } while (bytesRead > 0);

                            //await targetStream.WriteAsync(section.Body.);

                            var newFile = new FileModel(trustedFileNameForDisplay,
                                trustedFileNameForFileStorage, _targetFilePath, "", 0);
                            var affectedRows = _unitOfWork.Files.AddAsync(newFile);

                            // TODO: log
                            _logger.LogInformation(
                                "Uploaded file '{TrustedFileNameForDisplay}' saved to " +
                                "'{TargetFilePath}' as {TrustedFileNameForFileStorage}",
                                trustedFileNameForDisplay, _targetFilePath,
                                trustedFileNameForFileStorage);
                        }
                    }
                }
                /*
                    // Do not limit the key name length here because the 
					// multipart headers length limit is already in effect.
					var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
					var encoding = GetEncoding(section);
					using (var streamReader = new StreamReader(
						section.Body,
						encoding,
						detectEncodingFromByteOrderMarks: true,
						bufferSize: 1024,
						leaveOpen: true))
					{
						// The value length limit is enforced by MultipartBodyLengthLimit
						var value = await streamReader.ReadToEndAsync();
						if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
						{
							value = String.Empty;
						}
						formAccumulator.Append(key.Value, value); // For .NET Core <2.0 remove ".Value" from key

						if (formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
						{
							throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
						}
					}
                 */


                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            return Created(nameof(FilesController), null);
        }
        #endregion

        private static Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader =
                MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);

            // UTF-7 is insecure and shouldn't be honored. UTF-8 succeeds in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }
    }

    public class FormData
    {
        public string Note { get; set; }
    }
}
