# MovieRankerApp

MovieRankerApp is a web application that allows users to rank and review their favorite movies. Users can add movies, rate them, view a list of top-rated movies and share this list with their friends via email.

This app is deployed at 
[https://movierankerapp.onrender.com/](https://movierankerapp.onrender.com)

---

## Features

- **Create Movie**: Users can add new movies with details like name, genre, release date, and studio.
- **Score and Rate Movies**: Users can rate movies on a scale of 0 to 10.
- **View Rankings**: Users can view a list of movies sorted by their ratings.
- **User Authentication**: Users can create accounts and log in to manage their movie rankings.
- **Email list**: Users can share their movie list with their friends via email

---

## Technologies Used

- **Frontend**: ASP.NET Core MVC, Razor Pages, bootstrap, bootswatch theme
- **Backend**: ASP.NET Core MVC, Entity Framework Core, Email SMTP
- **Database**: Supabase PostgreSQL
- **Authentication**: ASP.NET Core Identity
- **Deployment**: Render.com
- **Extras**: Cron-job set up to spin render.com sever before shutdown as free version shuts down after 15 mins of inactivity
---
