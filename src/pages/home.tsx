import './home.css';
import CtaButton from '../components/home/CtaButton';
import { discordLink, githubLink } from '../constants';
import { faDiscord, faGithub } from '@fortawesome/free-brands-svg-icons';
import { faBookBookmark } from '@fortawesome/free-solid-svg-icons';

export default function HomePage() {
  return (
    <div>
      <div className='min-h-screen pt-12 mb-4 bg-cover bg-black flex items-center' style={{
        backgroundImage: `url('/homepage-bg.jpg')`,
      }}>
        <div className='container mx-auto px-8 py-4 flex flex-col-reverse md:flex-row'>
          <div className='flex flex-col text-white my-auto mr-0 md:mr-10 lg:mr-40'>
            <div className='hero-font italic text-[2.75rem] lg:text-[4rem]'>
              Dedicated to the pursuit and preservation of knowledge.
            </div>
            <div className='text-2xl mt-4'>
              We're a community of developers who are passionate about the critically acclaimed MMORPG Final Fantasy
              XIV.
            </div>
            <div className='mt-16 flex flex-wrap'>
              <CtaButton text='Discord' href={discordLink} faIcon={faDiscord} />
              <CtaButton text='GitHub' href={githubLink} faIcon={faGithub} />
              <CtaButton text='Wiki' href='/wiki' faIcon={faBookBookmark} />
            </div>
          </div>

          <img src='/logo.svg' alt=''
               className='mx-auto mb-20 md:mb-0 md:mx-0 sm:ml-auto w-60 h-60 sm:w-72 sm:h-72 lg:w-96 lg:h-96' />
        </div>
      </div>
    </div>
  );
}