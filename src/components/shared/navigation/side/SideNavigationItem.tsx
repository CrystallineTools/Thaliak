import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faAngleRight, IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { ReactElement } from 'react';
import Link from '../../Link';

export default function SideNavigationItem({ customIcon, faIcon, text, href }: {
  customIcon?: ReactElement,
  faIcon?: IconDefinition,
  text: string,
  href: string
}) {
  return (
    <ul className='space-y-2'>
      <li>
        <Link href={href} className='flex items-center px-4 py-3 text-base rounded-lg hover:bg-gray-200'>
          <div className='w-6 h-6 text-center'>
            {customIcon && customIcon}
            {faIcon && <FontAwesomeIcon icon={faIcon} />}
          </div>
          <span className='ml-3'>{text}</span>
          <FontAwesomeIcon icon={faAngleRight} className='w-3 h-3 pl-3 ml-auto' />
        </Link>
      </li>
    </ul>
  );
}
