namespace DevHub.Utilities;
public static class UploadFile
{
    public async static Task<string> UploadImageFileToServerAsync(IFormFile image, UploadFilePurpose purpose, string webRootPath)
    {
        try
        {
            string imageName = Guid.NewGuid().ToString() + Path.GetFileName(image.FileName);

            string basePath = purpose switch
            {
                UploadFilePurpose.USER_PROFILE_IMG => $"{webRootPath}" + "/users_headers_images/" + imageName,
                UploadFilePurpose.POST_MAIN_IMG => $"{webRootPath}" + "/posts_headers_images/" + imageName,
                UploadFilePurpose.POST_RELEATED_IMG => $"{webRootPath}" + "/posts_images/" + imageName,
            };

            string imagePath = Path.Combine(basePath, imageName);

            using (var fs = new FileStream(imagePath, FileMode.Create))
                await image.CopyToAsync(fs);

            return imagePath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
            return string.Empty;
        }
    }
}

public enum UploadFilePurpose
{
    USER_PROFILE_IMG,
    POST_MAIN_IMG,
    POST_RELEATED_IMG
}

