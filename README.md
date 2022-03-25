# Tugas Besar 2 IF2211 Strategi Algoritma
# Tree Explorer
> Sebuah aplikasi GUI desktop untuk melakukan folder crawling menggunakan algoritma pencarian BFS dan DFS

## Table of Contents
* [General Info](#general-information)
* [Technologies Used](#technologies-used)
* [Features](#features)
* [Setup](#setup)
* [Usage](#usage)
* [How to Build](#how-to-build)
* [Project Status](#project-status)
* [Author](#author)
<!-- * [License](#license) -->


## General Information
- Aplikasi ini dibuat untuk memodelkan fitur dari file explorer pada sistem operasi, bisa disebut juga sebagai Folder Crawling
- Folder-folder ditelusuri dengan memanfaatkan algoritma Breadth First Search (BFS) dan Depth First Search (DFS)


## Technologies Used
- C#
- .NET Core
- Windows Presentation Foundation (WPF)
- Visual Studio
- MSAGL untuk visualisasi graf

## Features

- Pengguna dapat memilih metode pencarian dengan BFS atau DFS
- Pengguna dapat memilih untuk melakukan pencarian sekali atau semua file yang dapat ditemukan
- Pengguna dapat memilih apakah visualisasi graf beranimasi atau tidak


## Setup
Agar dapat menjalankan file yang berada pada folder bin diperlukan instalasi
- [.NET Core versi 6](https://dotnet.microsoft.com/en-us/download)

## Usage

- Buka file Tree Explorer.exe yang berada di dalam folder bin
- Saat aplikasi telah terbuka, akan ada dua bagian, yaitu input dan output.
- Pada bagian input, tekan tombol Choose Folder untuk memilih direktori awal dilakukannya pencarian suatu file
- Pada bagian kolom Input File Name isi dengan nama file yang ingin dicari disertai dengan ekstensinya
- Checkboxes Find all occurence dapat dicentang apabila pengguna menginginkan pencarian untuk semua file yang memiliki nama sama dengan input File Name. Apabila dibiarkan tidak tercentang, pencarian akan berhenti ketika menemukan satu saja file yang bernama sama dengan input File Name
- Pada bagian Metode Pencarian, pengguna dapat memilih metode pencarian yang akan digunakan untuk mencari file di dalam direktori. Tersedia metode BFS dan DFS yang dapat dipilih oleh pengguna.
- Checkboxes Animated dapat dicentang apabila pengguna menginginkan output pencarian dilakukan secara bertahap (dengan animasi) sesuai dengan metode pencarian.
- Apabila Animate dicentang, pengguna dapat mengatur kecepatan munculnya setiap file atau folder pada bagian output dengan menggeser slider.
- Setelah semua input telah terpenuhi, tombol Search dapat diklik untuk memunculkan output pencarian.
- Pada hasil output pencarian, warna hitam pada node berarti node tersebut telah dibangkitkan, tetapi tidak dilakukan pemeriksaan. Warna merah menunjukkan node yang telah diperiksa, sedangkan warna biru menunjukkan jalur mulai dari direktori awal sampai file yang dicari ditemukan.
- Selain itu, terdapat pula waktu eksekusi dan path file yang dicari yang dapat dibuka dengan mengeklik tombol open.


## How to Build
Buka project pada "src/Tree Explorer" dengan Visual Studio (project dibuat dengan versi 2022). Pastikan memiliki .Net 6, serta menginstall MSAGL untuk WPF melalui Nugget. Tekan Build untuk melakukan kompilasi.

## Project Status
Project is: _complete_

## Author
| Nama                        | NIM      |
| --------------------------- | -------- |
| Tri Sulton Adila            | 13520033 |
| Bryan Amirul Husna          | 13520146 |
| Mohammad Hilmi Rinaldi      | 13520149 |