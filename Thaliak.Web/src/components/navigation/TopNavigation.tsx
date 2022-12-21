import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faDiscord, faGithub } from '@fortawesome/free-brands-svg-icons';
import { faBars, faCircleDollarToSlot } from '@fortawesome/free-solid-svg-icons';
import { Link } from 'react-router-dom';
import { discordLink, githubMainRepoLink, githubSponsorsLink } from '../../constants';
import cn from 'classnames';
import { useState } from 'react';
import logo from './logo.svg';

export default function TopNavigation() {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <nav
      className='fixed top-0 left-0 right-0 z-40 items-center justify-between sm:justify-start bg-slate-800 text-white'>
      <div className='container mx-auto flex flex-wrap sm:flex-row py-1.5 px-4'>
        <Link className='inline-block py-1 mr-4 text-xl text-gray-100' to='/'>
          <img
            alt='logo'
            src={logo}
            width='30'
            height='30'
            className='inline-block align-top'
          />{' '}
          Thaliak
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
            <li className='py-2 sm:py-0'>
              <Link role='button' className='text-gray-400 hover:text-gray-100 sm:px-2' to='/'>Repositories</Link>
            </li>

            <li className='py-2 sm:py-0'>
              <Link className='text-gray-400 hover:text-gray-100 sm:px-2'
                 id='main-nav-dropdown-api-docs'
                 to='/graphql/'>
                GraphQL API
              </Link>
            </li>

          </ul>
          <div className='ml-auto mt-3 sm:mt-0'>
            <a href={discordLink} className='p-2 text-gray-400 hover:text-gray-100'>
              <FontAwesomeIcon icon={faDiscord} />
            </a>
            <a href={githubSponsorsLink} className='p-2 text-gray-400 hover:text-gray-100'>
              <FontAwesomeIcon icon={faCircleDollarToSlot} />
            </a>
            <a href={githubMainRepoLink} className='p-2 text-gray-400 hover:text-gray-100'>
              <FontAwesomeIcon icon={faGithub} />
            </a>
          </div>
        </div>
      </div>
    </nav>
  );
}
