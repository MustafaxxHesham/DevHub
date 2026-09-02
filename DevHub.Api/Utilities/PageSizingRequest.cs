using Microsoft.AspNetCore.Mvc;
namespace DevHub.Utilities;
public record PageSizingRequest([FromHeader(Name = "X-Page-Size")] int pageSize, [FromHeader(Name = "X-Page-Number")] int pageNumber);