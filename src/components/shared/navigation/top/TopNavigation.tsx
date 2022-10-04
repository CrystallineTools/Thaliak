import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faDiscord, faGithub } from '@fortawesome/free-brands-svg-icons';
import { faBars, faBookBookmark } from '@fortawesome/free-solid-svg-icons';
import { useLocation } from 'react-router-dom';
import { discordLink, githubLink } from '../../../../constants';
import cn from 'classnames';
import { useState } from 'react';
import ewerLogo from '../ewer-logo.svg';
import thaliakLogo from '../thaliak-logo.svg';
import TopNavigationItem from './TopNavigationItem';
import Link from '../../Link';

export default function TopNavigation() {
  const [isOpen, setIsOpen] = useState(false);
  const isHomePage = useLocation().pathname === '/';

  return (
    <nav
      className={cn('fixed top-0 left-0 right-0 z-40 items-center justify-between sm:justify-start text-white',
        { 'bg-stone-800': !isHomePage })}>
      <div className='container mx-auto flex flex-wrap sm:flex-row py-1.5 px-4'>
        <Link className='inline-block py-1 mr-4 text-xl text-gray-100' href='/'>
          <img
            alt='logo'
            src={ewerLogo}
            className='inline-block align-top mr-2 w-7 h-7'
          />
          The Ewer
        </Link>
        <button
          className='inline-block py-1 px-2 leading-none bg-transparent border border-transparent rounded text-gray-400 border-gray-600 ml-auto sm:hidden'
          aria-controls='main-nav'
          aria-label='Toggle navigation'
          onClick={() => setIsOpen(!isOpen)}>
          <FontAwesomeIcon icon={faBars} />
        </button>
        <div
          className={cn('collapse navbar-collapse w-full flex-grow items-center sm:flex sm:w-auto', { hidden: !isOpen })}
          id='main-nav'>

          <ul className='flex flex-col pl-0 mb-0 list-none mr-auto sm:flex-row'>
            <TopNavigationItem
              text='Thaliak'
              href='/thaliak'
              icon={
                <img alt='' src={thaliakLogo} />
              } />

            <TopNavigationItem
              text='Wiki'
              href='/wiki'
              icon={<FontAwesomeIcon icon={faBookBookmark} />} />
          </ul>

          {!isHomePage && (
            <div className='ml-auto mt-3 sm:mt-0'>
              <a href={discordLink} className='p-2 text-gray-400 hover:text-gray-100'>
                <FontAwesomeIcon icon={faDiscord} />
              </a>
              <a href={githubLink} className='p-2 text-gray-400 hover:text-gray-100'>
                <FontAwesomeIcon icon={faGithub} />
              </a>
            </div>
          )}
        </div>
      </div>
    </nav>
  );
}
