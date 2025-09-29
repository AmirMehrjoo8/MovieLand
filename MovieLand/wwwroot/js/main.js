"use strict";

const apiKey = "3778bd430a6b5a35d815d210850d537a";
const format = 'movie';
const ID = 213;
const language = 'en-US';
let genresID;

const NewesMovieswrapper = document.querySelector('.Newest-Movies-Wrapper');
const BestMoviesWrapper = document.querySelector('.best-moveis-wrapper');
const BestShowsWrapper = document.querySelector('.best-shows-wrapper');
const AdventureMoviesWrapper = document.querySelector('.Adventure-movies-wrapper');
const actionMoviesWrapper = document.querySelector('.action-movies-wrapper');
const herrorMoviesWrapper = document.querySelector('.herror-movies-wrapper');
const rightSection = document.querySelector('.right-section');
const leftSection = document.querySelector('.left-section');
const singlePageHeader = document.querySelector('.singlePage-header');
const downloadContainer = document.querySelector('.download-section');
const relatedMoviesWrapper = document.querySelector('.related-movies-wrapper');
const replysShow = document.querySelector('.reply-show')

const genres = function (arr) {
  const { genres } = arr;
  const [fGenres] = genres;
  const genreNames = genres.map(g => g.name).join(', ');
  return genreNames;
}
const resGenresID = function (arr) {
  const { genres } = arr;
  const [fGenres] = genres;
  genresID = fGenres.id;
  return genresID;
}


const countrys = function (arr) {
  const { production_countries } = arr;
  const countrysName = production_countries.map(g => g.name).join(', ');
  return countrysName;
}



const renderSinglePagePoster = async function () {
  const res = await fetch(`https://api.themoviedb.org/3/${format}/${ID}?api_key=${apiKey}`);
  const data = await res.json();
  const poster = document.createElement('div');
  poster.classList.add('poster-container');
  console.log(data)
  poster.innerHTML = `
        <img src="https://image.tmdb.org/t/p/w342${data.poster_path}" alt="" class="poster">
    `;
  rightSection.appendChild(poster);

  const background = document.createElement('img');
  background.src = `https://image.tmdb.org/t/p/original${data.backdrop_path}`;
  background.classList.add('bg-img');
  console.log(data)
  singlePageHeader.appendChild(background);
}
renderSinglePagePoster();

const renderSinglePageDetails = async function () {
  const res = await fetch(`https://api.themoviedb.org/3/${format}/${ID}?api_key=${apiKey}&language=${language}`);
  const data = await res.json();

  const Details = document.createElement('div');
  Details.classList.add('movieDetails');

  const genresName = genres(data);
  const countrysName = countrys(data);

  const DetailsHTML = format === 'tv' ? `
                            <ul>
                        <li>
                            <span>نام : ${data.name || data.title}</span>
                        </li>
                        <li>
                            <span>ژانر : ${genresName}</span>
                        </li>
                        <li>
                            <span>نمره : ${data.vote_average} </span>
                        </li>
                        <li>
                            <span>محصول کشور: ${countrysName}</span>
                        </li>

                          <li><span>تعداد فصل ها : ${data.number_of_seasons}</span></li>
                        <li>
                            <span>تعداد قسمت ها: ${data.number_of_episodes}</span>
                        </li>
                        <li>
                            <span>خلاصه داستان: ${data.overview || 'ندارد'}</span>
                        </li>
                    </ul>
    ` : `
                            <ul>
                        <li>
                            <span>نام : ${data.name || data.title}</span>
                        </li>
                        <li>
                            <span>ژانر : ${genresName}</span>
                        </li>
                        <li>
                            <span>نمره : ${data.vote_average} </span>
                        </li>
                        <li>
                            <span>محصول کشور: ${countrysName}</span>
                        <li>
                            <span>خلاصه داستان: ${data.overview || 'ندارد'}</span>
                        </li>
                    </ul>
    `;


  Details.innerHTML = DetailsHTML;

  leftSection.appendChild(Details);
}

renderSinglePageDetails()

const renderSeasonsEpisodes = async function (tvID) {
  try {
    const res = await fetch(`https://api.themoviedb.org/3/tv/${tvID}?api_key=${apiKey}&language=fa-IR`);
    const data = await res.json();
    if (format === "tv") {
      for (const season of data.seasons) {
        if (season.season_number === 0) continue;


        const seasonWrapper = document.createElement('div');
        seasonWrapper.classList.add('season-wrapper');

        seasonWrapper.innerHTML = `
        <div class="season-header">فصل ${season.season_number}</div>
        <div class="season-body" style="display: none;"></div>
      `;

        downloadContainer.appendChild(seasonWrapper);

        const seasonBody = seasonWrapper.querySelector('.season-body');


        const header = seasonWrapper.querySelector('.season-header');
        header.addEventListener('click', () => {
          const visible = seasonBody.style.display === 'block';
          seasonBody.style.display = visible ? 'none' : 'block';
        });


        const seasonRes = await fetch(`https://api.themoviedb.org/3/tv/${tvID}/season/${season.season_number}?api_key=${apiKey}&language=fa-IR`);
        const seasonData = await seasonRes.json();

        seasonData.episodes.forEach(episode => {
          const episodeBox = document.createElement('div');
          episodeBox.classList.add('episode-box');
          episodeBox.innerHTML = `
          <h4>قسمت ${episode.episode_number}:</h4>
          <div class="quality-buttons">
            <a href="#" class="quality-btn">480p</a>
            <a href="#" class="quality-btn">720p</a>
            <a href="#" class="quality-btn">1080p</a>
          </div>
        `;
          seasonBody.appendChild(episodeBox);
        });
      }
    } else {
      const elem = document.createElement('div');
      elem.classList.add('movie-download');
      elem.innerHTML = `
                <a href="">دانلود با کیفیت 480p</a>
                <a href="">دانلود با کیفیت 720p</a>
                <a href="">دانلود با کیفیت 1080p</a>
    `;

      downloadContainer.appendChild(elem)

    }

  } catch (err) {
    console.error('خطا در دریافت قسمت‌ها:', err);
  }
};

renderSeasonsEpisodes(ID)

let renderRelatedMovies = async function () {
  try {
    const res1 = await fetch(`https://api.themoviedb.org/3/${format}/${ID}?api_key=${apiKey}&language=fa-IR`);
    const data1 = await res1.json();
    const { genres } = data1;
    const genres1 = genres[0].id;

    const res = await fetch(`https://api.themoviedb.org/3/discover/${format}?api_key=${apiKey}&sort_by=vote_average.desc&with_genres=${genres1}&vote_count.gte=100`);
    const data = await res.json();
    data.results.forEach(movie => {
      if (movie.backdrop_path) {
        const slide = document.createElement('div');
        slide.classList.add('swiper-slide', 'related-movies-slide');
        slide.innerHTML = `
        <a href="/ID=${movie.ID}/format=movie"> <img src="https://image.tmdb.org/t/p/w342${movie.poster_path}" alt="${movie.name}"></a>
      `;
        relatedMoviesWrapper.appendChild(slide);

      }
    });

    const relatedMoviesSwiper = new Swiper(".related-movies-swiper", {
      slidesPerView: 5,
      centeredslides: false,
      spaceBetween: 50,
      pagination: {
        el: ".related-movies-swiper .swiper-pagination",
        type: "fraction",
      },
      navigation: {
        nextEl: ".related-movies-swiper .swiper-button-next",
        prevEl: ".related-movies-swiper .swiper-button-prev",
      },

    });
  } catch (err) {
    console.error('خطا در دریافت اطلاعات', err)
  }
}

renderRelatedMovies();



document.querySelectorAll('.reply-show').forEach(button => {
  button.addEventListener('click', function (e) {
    e.preventDefault();

    const comment = this.closest('.comment');

    const replys = comment.querySelector('.replys');

    if (replys.style.display === 'none' || !replys.style.display) {
      replys.style.display = 'flex';
      this.querySelector('svg').classList.add('rotate');
    } else {
      replys.style.display = 'none';
      this.querySelector('svg').classList.remove('rotate');
    }
  });
});

const roleCheckbox = document.querySelector('.roles-checkbox');
const registedbtn = document.querySelector('.registedbtn');

if(!roleCheckbox.checked) {
  registedbtn.setAttribute('disabled', '');
}

roleCheckbox.addEventListener('change', function () {
  if (!roleCheckbox.checked) {
        registedbtn.setAttribute('disabled', '');
  } else {

    registedbtn.removeAttribute('disabled');
  }
});

tex