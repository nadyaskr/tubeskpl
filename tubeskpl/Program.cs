using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

class Book
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Category { get; set; }
    public int Year { get; set; }
    public bool IsAvailable => Borrower == null;
    public string Borrower { get; set; }
    public DateTime? BorrowedAt { get; set; }
}

class Program
{
    static List<Book> books = new List<Book>
    {
        new Book { Id = "B01", Title = "Algoritma", Author = "Dian", Category = "Teknik", Year = 2019 },
        new Book { Id = "B02", Title = "Pemrograman C#", Author = "Andi", Category = "Komputer", Year = 2021 },
        new Book { Id = "B03", Title = "Sejarah Dunia", Author = "Rini", Category = "Sejarah", Year = 2018 },
        new Book { Id = "B04", Title = "Naruto Vol. 1", Author = "Masashi Kishimoto", Category = "Komik", Year = 2000 },
        new Book { Id = "B05", Title = "One Piece Vol. 5", Author = "Eiichiro Oda", Category = "Komik", Year = 2002 },
        new Book { Id = "B06", Title = "Atomic Habits", Author = "James Clear", Category = "Self-Improvement", Year = 2018 },
        new Book { Id = "B07", Title = "Harry Potter", Author = "J.K. Rowling", Category = "Fiksi", Year = 1997 },
        new Book { Id = "B08", Title = "Filosofi Teras", Author = "Henry Manampiring", Category = "Psikologi", Year = 2019 },
        new Book { Id = "B09", Title = "Laskar Pelangi", Author = "Andrea Hirata", Category = "Novel", Year = 2005 },
        new Book { Id = "B10", Title = "The Pragmatic Programmer", Author = "David Thomas", Category = "Komputer", Year = 1999 },
    };

    // table driven
    static Dictionary<string, Action> menuActions = new Dictionary<string, Action>
    {
        { "1", TampilkanBuku },
        { "2", PinjamBuku },
        { "3", KembalikanBuku },
        { "4", CariBuku },
        { "0", () => Environment.Exit(0) }
    };

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n=== Menu Mahasiswa ===");
            Console.WriteLine("1. Lihat semua buku");
            Console.WriteLine("2. Pinjam buku");
            Console.WriteLine("3. Kembalikan buku");
            Console.WriteLine("4. Cari buku");
            Console.WriteLine("0. Keluar");
            Console.Write("Pilih menu: ");
            string pilihan = Console.ReadLine();

            if (menuActions.ContainsKey(pilihan))
                menuActions[pilihan]();
            else
                Console.WriteLine("Pilihan tidak valid.");
        }
    }

    static void TampilkanBuku()
    {
        Console.WriteLine("\nDaftar Buku:");
        foreach (var book in books)
        {
            Console.WriteLine($"ID: {book.Id}, Judul: {book.Title}, Penulis: {book.Author}, Kategori: {book.Category}, Tahun: {book.Year}, Status: {(book.IsAvailable ? "Tersedia" : "Dipinjam")}");
        }
    }

    static void PinjamBuku()
    {
        Console.Write("Masukkan ID buku yang ingin dipinjam: ");
        string id = Console.ReadLine()?.Trim().ToUpper();

        var book = books.FirstOrDefault(b => b.Id == id);
        if (book == null || !book.IsAvailable)
        {
            Console.WriteLine("Buku tidak ditemukan atau sedang dipinjam.");
            return;
        }

        Console.Write("Masukkan nama peminjam: ");
        string nama = Console.ReadLine()?.Trim();

        book.Borrower = nama;
        book.BorrowedAt = DateTime.Now;

        Console.WriteLine("\n--- Bukti Peminjaman ---");
        Console.WriteLine($"Judul: {book.Title}, Dipinjam oleh: {nama}, Tanggal: {book.BorrowedAt.Value.ToShortDateString()}");
    }

    static void KembalikanBuku()
    {
        Console.Write("Masukkan ID buku yang ingin dikembalikan: ");
        string id = Console.ReadLine()?.Trim().ToUpper();

        var book = books.FirstOrDefault(b => b.Id == id);
        if (book == null || book.IsAvailable)
        {
            Console.WriteLine("Buku tidak ditemukan atau belum dipinjam.");
            return;
        }

        Console.Write("Masukkan nama pengembali: ");
        string nama = Console.ReadLine()?.Trim();
        if (nama != book.Borrower)
        {
            Console.WriteLine($"Buku ini dipinjam oleh {book.Borrower}, bukan {nama}.");
            return;
        }

        Console.Write("Masukkan tanggal pengembalian: ");
        string inputDate = Console.ReadLine()?.Trim();

        // Mencoba mengubah input tanggal ke DateTime
        DateTime returnDate;
        bool isValidDate = TryParseFlexibleDate(inputDate, out returnDate);

        if (!isValidDate)
        {
            Console.WriteLine("Tanggal pengembalian tidak valid.");
            return;
        }

        DateTime borrowedDate = book.BorrowedAt.Value;
        int lateDays = (returnDate - borrowedDate).Days;
        string statusPengembalian = lateDays <= 0 ? "Pengembalian tepat waktu." : $"Telat {lateDays} hari.";

        Console.WriteLine("\n--- Bukti Pengembalian ---");
        Console.WriteLine($"Judul: {book.Title}, Dikembalikan oleh: {nama}, Tanggal: {returnDate.ToShortDateString()}");
        Console.WriteLine(statusPengembalian);

        // Reset status peminjaman
        book.Borrower = null;
        book.BorrowedAt = null;
    }

    static void CariBuku()
    {
        Console.Write("Kata kunci (judul/penulis, opsional): ");
        string keyword = Console.ReadLine()?.Trim().ToLower();
        Console.Write("Kategori (opsional): ");
        string kategori = Console.ReadLine()?.Trim().ToLower();
        Console.Write("Hanya tampilkan buku yang tersedia? (y/n): ");
        string hanyaTersedia = Console.ReadLine()?.Trim().ToLower();

        var hasil = books.Where(b =>
            (string.IsNullOrEmpty(keyword) || b.Title.ToLower().Contains(keyword) || b.Author.ToLower().Contains(keyword)) &&
            (string.IsNullOrEmpty(kategori) || b.Category.ToLower() == kategori) &&
            (hanyaTersedia != "y" || b.IsAvailable));

        Console.WriteLine("\nHasil Pencarian:");
        foreach (var b in hasil)
        {
            Console.WriteLine($"ID: {b.Id}, Judul: {b.Title}, Penulis: {b.Author}, Kategori: {b.Category}, Status: {(b.IsAvailable ? "Tersedia" : "Dipinjam")}");
        }
    }

    // parameterization
    static bool TryParseFlexibleDate(string dateString, out DateTime result)
    {
        string[] formats = new string[]
        {
            "dd/MM/yyyy"
        };

        result = DateTime.MinValue;
        return DateTime.TryParseExact(dateString, formats, new CultureInfo("id-ID"), DateTimeStyles.None, out result);
    }
}
