using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;
using VinylShop.DbModel;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace VinylShop.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly VinylShopContext _context;

        public AlbumsController(VinylShopContext context)
        {
            _context = context;
        }

        // List Albums with Sorting
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["TitleSortParam"] = sortOrder == "title_asc" ? "title_desc" : "title_asc";
            ViewData["ArtistSortParam"] = sortOrder == "artist_asc" ? "artist_desc" : "artist_asc";
            ViewData["GenreSortParam"] = sortOrder == "genre_asc" ? "genre_desc" : "genre_asc";

            var albums = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .AsQueryable();

            albums = ApplySorting(albums, sortOrder);
            return View(await albums.ToListAsync());
        }

        // View Album Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var album = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (album == null) return NotFound();

            return View(album);
        }

        // Create Album - GET
        public IActionResult Create()
        {
            PopulateDropDowns();
            return View();
        }

        // Create Album - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Album album, IFormFile? ImageFile, IFormFile? ArtistImageFile, string ExistingArtist, string NewArtist, string GenreId, string NewGenre, List<string> SongTitles, List<string> SongDurations)
        {
            // Assign Artist
            var artist = await GetOrCreateArtist(ExistingArtist, NewArtist, ArtistImageFile);
            if (artist == null)
            {
                ModelState.AddModelError("Artist", "Please select or enter an artist.");
                PopulateDropDowns();
                return View(album);
            }
            album.Artist = artist;

            // Assign Genre
            int genreId = await GetOrCreateGenre(GenreId, NewGenre);
            if (genreId == 0)
            {
                ModelState.AddModelError("Genre", "Please select or enter a genre.");
                PopulateDropDowns();
                return View(album);
            }
            album.GenreId = genreId;

            // Image Upload
            album.ImagePath = await UploadImageAsync(ImageFile, "albums");

            // Create Songs
            if (SongTitles != null && SongDurations != null)
            {
                for (int i = 0; i < SongTitles.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(SongTitles[i]) && !string.IsNullOrWhiteSpace(SongDurations[i]))
                    {
                        album.Songs.Add(new Song
                        {
                            Title = SongTitles[i],
                            Duration = TimeSpan.TryParse(SongDurations[i], out TimeSpan duration) ? duration : TimeSpan.Zero
                        });
                    }
                }
            }

            // Save Album
            _context.Add(album);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Edit Album - GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var album = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (album == null) return NotFound();

            var albumDto = new AlbumEditDto
            {
                Id = album.Id,
                Title = album.Title,
                ArtistId = album.ArtistId,
                GenreId = album.GenreId,
                ImagePath = album.ImagePath,
                ArtistImagePath = album.Artist?.ImagePath,
                Songs = album.Songs.Select(s => new SongDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Duration = s.Duration.ToString(@"hh\:mm\:ss")
                }).ToList()
            };


            ViewBag.Artists = new SelectList(_context.Artists, "Id", "Name", album.ArtistId);
            ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name", album.GenreId);

            return View(albumDto);
        }
        
        // Edit Album - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AlbumEditDto albumDto)
        {
            if (id != albumDto.Id) return NotFound();

            var album = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Artists = new SelectList(_context.Artists, "Id", "Name", albumDto.ArtistId);
                ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name", albumDto.GenreId);
                return View(albumDto);
            }

            try
            {
                // Оновлення загальних даних альбому
                album.Title = albumDto.Title;
                album.ArtistId = albumDto.ArtistId;
                album.GenreId = albumDto.GenreId;

                if (albumDto.ImageFile != null)
                {
                    album.ImagePath = await UploadImageAsync(albumDto.ImageFile, "albums");
                }

                if (albumDto.ArtistImageFile != null && album.Artist != null)
                {
                    album.Artist.ImagePath = await UploadImageAsync(albumDto.ArtistImageFile, "artists");
                }

                var existingSongs = album.Songs.ToList();
                albumDto.Songs ??= new List<SongDto>();

                foreach (var songDto in albumDto.Songs)
                {
                    if (songDto.Id.HasValue)
                    {
                        // Оновлення існуючої пісні
                        var song = existingSongs.FirstOrDefault(s => s.Id == songDto.Id.Value);
                        if (song != null)
                        {
                            song.Title = songDto.Title;
                            song.Duration = TimeSpan.TryParse(songDto.Duration, out TimeSpan duration) ? duration : TimeSpan.Zero;
                        }
                    }
                    else
                    {
                        // Додавання нової пісні
                        album.Songs.Add(new Song
                        {
                            Title = songDto.Title,
                            Duration = TimeSpan.TryParse(songDto.Duration, out TimeSpan duration) ? duration : TimeSpan.Zero
                        });
                    }
                }

                // Видалення пісень, яких більше немає в DTO
                var songIds = albumDto.Songs.Where(s => s.Id.HasValue).Select(s => s.Id ?? 0).ToList();
                var songsToRemove = existingSongs.Where(s => !songIds.Contains(s.Id)).ToList();

                if (songsToRemove.Any())
                {
                    _context.Songs.RemoveRange(songsToRemove);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating album: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating the album.");
                ViewBag.Artists = new SelectList(_context.Artists, "Id", "Name", albumDto.ArtistId);
                ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name", albumDto.GenreId);
                return View(albumDto);
            }
        }

        // Delete Album - GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var album = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);

            return album == null ? NotFound() : View(album);
        }

        // Delete Album - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album != null)
            {
                _context.Albums.Remove(album);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Sorting Logic
        private IQueryable<Album> ApplySorting(IQueryable<Album> albums, string sortOrder)
        {
            return sortOrder switch
            {
                "title_desc" => albums.OrderByDescending(a => a.Title),
                "title_asc" => albums.OrderBy(a => a.Title),
                "artist_desc" => albums.OrderByDescending(a => a.Artist.Name),
                "artist_asc" => albums.OrderBy(a => a.Artist.Name),
                "genre_desc" => albums.OrderByDescending(a => a.Genre.Name),
                "genre_asc" => albums.OrderBy(a => a.Genre.Name),
                _ => albums.OrderBy(a => a.Title)
            };
        }

        // Dropdowns for Artists & Genres
        private void PopulateDropDowns()
        {
            ViewBag.Artists = new SelectList(_context.Artists, "Id", "Name");
            ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name");
        }

        // Get or Create Artist
        private async Task<Artist?> GetOrCreateArtist(string existingArtistId, string newArtistName, IFormFile? artistImageFile)
        {
            if (!string.IsNullOrWhiteSpace(existingArtistId) && int.TryParse(existingArtistId, out int artistId))
            {
                return await _context.Artists.FindAsync(artistId);
            }
            else if (!string.IsNullOrWhiteSpace(newArtistName))
            {
                var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Name.ToLower() == newArtistName.Trim().ToLower());
                if (artist == null)
                {
                    artist = new Artist { Name = newArtistName.Trim() };

                    if (artistImageFile != null)
                    {
                        artist.ImagePath = await UploadImageAsync(artistImageFile, "artists");
                    }

                    _context.Artists.Add(artist);
                    await _context.SaveChangesAsync();
                }
                return artist;
            }
            return null;
        }

        // Get or Create Genre
        private async Task<int> GetOrCreateGenre(string genreId, string newGenreName)
        {
            if (!string.IsNullOrWhiteSpace(genreId) && int.TryParse(genreId, out int parsedGenreId))
            {
                return parsedGenreId;
            }
            else if (!string.IsNullOrWhiteSpace(newGenreName))
            {
                var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name.ToLower() == newGenreName.Trim().ToLower());
                if (genre == null)
                {
                    genre = new Genre { Name = newGenreName.Trim() };
                    _context.Genres.Add(genre);
                    await _context.SaveChangesAsync();
                }
                return genre.Id;
            }
            return 0;
        }

        // Image Upload
        private async Task<string?> UploadImageAsync(IFormFile? imageFile, string folderName)
        {
            if (imageFile == null || imageFile.Length == 0) return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/img/{folderName}");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/img/{folderName}/" + uniqueFileName;
        }
    }
}
