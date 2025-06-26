// BEFORE

private async Task UpdateBrandData(BrandDto brandDto, Brand brand)
{
    ...

    if (!string.IsNullOrEmpty(brandDto.Picture))
    {
        var picture = await LoadPicture(brandDto.Picture, brand.Name, brand.PictureId);
        if (picture != null)
            brand.PictureId = picture.Id;
    }

    ...
}

private async Task<Picture> LoadPicture(string picturePath, string name, string picId = "")
{
    if (string.IsNullOrEmpty(picturePath) || !File.Exists(picturePath))
        return null;

    ...

    var newPicture = await _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(name));
    return newPicture;
}

// AFTER

private async Task UpdateBrandData(BrandDto brandDto, Brand brand)
{
    ...

    if (!string.IsNullOrEmpty(brandDto.Picture))
    {
        var pictureId = await LoadPictureId(brandDto.Picture, brand.Name, brand.PictureId);
        if (pictureId != null)
            brand.PictureId = pictureId;
    }

    ...
}

private async Task<string> LoadPictureId(string picturePath, string name, string picId = "")
{
    if (string.IsNullOrEmpty(picturePath) || !File.Exists(picturePath))
        return null;

    ...

    var newPicture = await _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(name));
    return newPicture.Id;
}