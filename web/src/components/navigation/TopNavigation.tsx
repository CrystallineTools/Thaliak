import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faDiscord, faGithub } from '@fortawesome/free-brands-svg-icons';
import { faBars, faCircleDollarToSlot, faXmark, faUser, faSignOutAlt, faLink } from '@fortawesome/free-solid-svg-icons';
import { Link } from 'react-router';
import { discordLink, githubMainRepoLink, githubSponsorsLink } from '../../constants';
import cn from 'classnames';
import { useState } from 'react';
import logo from './logo.svg';
import { API_BASE_URL } from '../../api/config';
import { logout } from '../../api/authClient';
import { useAuth } from '../../contexts/AuthContext';

export default function TopNavigation() {
  const [isOpen, setIsOpen] = useState(false);
  const [showUserMenu, setShowUserMenu] = useState(false);
  const { user, refreshAuth } = useAuth();

  const handleSignIn = () => {
    window.location.href = `${API_BASE_URL}/auth/discord/init`;
  };

  const handleSignOut = async () => {
    try {
      await logout();
      await refreshAuth();
      setShowUserMenu(false);
    } catch (error) {
      console.error('Failed to sign out:', error);
      alert('Failed to sign out. Please try again.');
    }
  };

  return (
    <nav className='fixed top-0 left-0 right-0 z-40 bg-linear-to-r from-primary-700 via-primary-600 to-primary-700 shadow-lg backdrop-blur-xs'>
      <div className='container mx-auto flex flex-wrap sm:flex-row py-3 px-4 items-center'>
        <Link className='inline-flex items-center gap-2 py-1 mr-6 text-xl font-semibold text-white hover:text-primary-100 transition-colors' to='/'>
          <img
            alt='logo'
            src={logo}
            width='32'
            height='32'
            className='inline-block'
          />
          <span>Thaliak</span>
        </Link>

        <button
          className='inline-flex items-center justify-center w-10 h-10 ml-auto sm:hidden text-white hover:bg-primary-500 rounded-lg transition-colors'
          aria-controls='main-nav'
          aria-label='Toggle navigation'
          onClick={() => setIsOpen(!isOpen)}>
          <FontAwesomeIcon icon={isOpen ? faXmark : faBars} className='text-lg' />
        </button>

        <div
          className={cn('w-full grow items-center sm:flex sm:w-auto', { hidden: !isOpen })}
          id='main-nav'>
          <ul className='flex flex-col mt-4 sm:mt-0 gap-1 sm:gap-0 pl-0 mb-0 list-none mr-auto sm:flex-row'>
            <li>
              <Link
                className='block px-4 py-2 text-white/90 hover:text-white hover:bg-primary-500 rounded-lg transition-all sm:rounded-md'
                to='/'
                onClick={() => setIsOpen(false)}>
                Repositories
              </Link>
            </li>
            <li>
              <Link
                className='block px-4 py-2 text-white/90 hover:text-white hover:bg-primary-500 rounded-lg transition-all sm:rounded-md'
                to='/graphql/'
                onClick={() => setIsOpen(false)}>
                GraphQL API (v1)
              </Link>
            </li>
            <li>
              <a
                className='block px-4 py-2 text-white/90 hover:text-white hover:bg-primary-500 rounded-lg transition-all sm:rounded-md inline-flex items-center gap-2'
                href={API_BASE_URL}
                target='_blank'
                rel='noopener noreferrer'
                onClick={() => setIsOpen(false)}>
                REST API (v2)
                <span className='inline-flex items-center px-1.5 py-0.5 text-[10px] font-bold uppercase bg-amber-400 text-amber-900 rounded'>
                  Beta
                </span>
              </a>
            </li>
          </ul>

          <div className='flex gap-1 mt-4 sm:mt-0 pb-2 sm:pb-0 items-center'>
            <a
              href={discordLink}
              className='inline-flex items-center justify-center w-10 h-10 text-white/90 hover:text-white hover:bg-primary-500 rounded-lg transition-all'
              aria-label='Discord'>
              <FontAwesomeIcon icon={faDiscord} className='text-lg' />
            </a>
            <a
              href={githubSponsorsLink}
              className='inline-flex items-center justify-center w-10 h-10 text-white/90 hover:text-white hover:bg-primary-500 rounded-lg transition-all'
              aria-label='GitHub Sponsors'>
              <FontAwesomeIcon icon={faCircleDollarToSlot} className='text-lg' />
            </a>
            <a
              href={githubMainRepoLink}
              className='inline-flex items-center justify-center w-10 h-10 text-white/90 hover:text-white hover:bg-primary-500 rounded-lg transition-all'
              aria-label='GitHub'>
              <FontAwesomeIcon icon={faGithub} className='text-lg' />
            </a>

            {user ? (
              <div className='relative ml-2'>
                <button
                  onClick={() => setShowUserMenu(!showUserMenu)}
                  className='inline-flex items-center gap-2 px-3 py-2 text-white/90 hover:text-white hover:bg-primary-500 rounded-lg transition-all'
                  aria-label='User menu'>
                  {user.discord_avatar ? (
                    <img
                      src={`https://cdn.discordapp.com/avatars/${user.discord_user_id}/${user.discord_avatar}.png?size=32`}
                      alt='Discord avatar'
                      className='w-6 h-6 rounded-full'
                    />
                  ) : (
                    <FontAwesomeIcon icon={faUser} />
                  )}
                  <span className='hidden sm:inline'>{user.discord_username}</span>
                </button>
                {showUserMenu && (
                  <div className='absolute right-0 mt-2 w-48 bg-white rounded-lg shadow-lg py-1 z-50'>
                    <Link
                      to='/webhooks'
                      className='flex items-center gap-2 px-4 py-2 text-gray-700 hover:bg-gray-100'
                      onClick={() => { setShowUserMenu(false); setIsOpen(false); }}>
                      <FontAwesomeIcon icon={faLink} />
                      <span>Manage Webhooks</span>
                    </Link>
                    <button
                      onClick={handleSignOut}
                      className='flex items-center gap-2 w-full px-4 py-2 text-left text-gray-700 hover:bg-gray-100'>
                      <FontAwesomeIcon icon={faSignOutAlt} />
                      <span>Sign Out</span>
                    </button>
                  </div>
                )}
              </div>
            ) : (
              <button
                onClick={handleSignIn}
                className='inline-flex items-center gap-2 px-4 py-2 ml-2 text-white bg-primary-500 hover:bg-primary-400 rounded-lg transition-all font-medium'>
                <FontAwesomeIcon icon={faDiscord} />
                <span className='hidden sm:inline'>Sign in</span>
              </button>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
