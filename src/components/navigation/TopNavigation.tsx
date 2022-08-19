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
  const [apiDropdownOpen, setApiDropdownOpen] = useState(false);

  return (
    <nav
      className='fixed top-0 left-0 right-0 z-40 flex flex-wrap sm:flex-row items-center justify-between sm:justify-start py-1.5 px-4 bg-slate-800 text-white'>
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
        className='inline-block py-1 px-2 leading-none bg-transparent border border-transparent rounded text-gray-400 border-gray-600 sm:hidden'
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
            <button className={`${apiDropdownOpen ? 'text-gray-100' : 'text-gray-400'} hover:text-gray-100 sm:px-2`}
                    id='main-nav-dropdown-api-docs'
                    onClick={() => setApiDropdownOpen(!apiDropdownOpen)}>
              API Docs
              <ul
                className={cn('min-w-max absolute bg-white text-base z-50 float-left list-none' +
                  'text-left rounded-md shadow-md mt-1 bg-clip-padding border-none', { hidden: !apiDropdownOpen })}>
                <li>
                  <a href='/graphql/'
                     className='text-sm text-left py-2 px-4 font-normal block whitespace-nowrap text-gray-700 hover:bg-gray-100'>
                    GraphQL API
                  </a>

                  <a href='/api/'
                     className='text-sm text-left py-2 px-4 font-normal block whitespace-nowrap text-gray-700 hover:bg-gray-100'>
                    <span
                      className='py-1 px-2 mr-1 text-white text-center font-semibold text-xs align-baseline rounded rounded-full bg-red-600'>
                      Deprecated
                    </span>
                    REST API
                  </a>
                </li>
              </ul>
            </button>
          </li>

        </ul>
        <div className='ml-auto mt-3 sm:mt-0'>
          <a href={discordLink} className='p-2'>
            <FontAwesomeIcon icon={faDiscord} />
          </a>
          <a href={githubSponsorsLink} className='p-2'>
            <FontAwesomeIcon icon={faCircleDollarToSlot} />
          </a>
          <a href={githubMainRepoLink} className='p-2'>
            <FontAwesomeIcon icon={faGithub} />
          </a>
        </div>
      </div>
    </nav>
  );
}
