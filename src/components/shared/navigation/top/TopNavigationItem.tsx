import { ReactElement } from 'react';
import Link from '../../Link';

export default function TopNavigationItem({ text, href, icon }: { text: string, href: string, icon?: ReactElement }) {
  return (
    <li className='py-2 sm:py-0'>
      <Link role='button' className='text-gray-400 hover:text-gray-100 sm:px-2' href={href}>
        {icon && (
          <span className='inline-block align-middle mr-2 w-5 h-5'>
            {icon}
          </span>
        )}
        <span className='align-middle'>
          {text}
        </span>
      </Link>
    </li>
  );
}