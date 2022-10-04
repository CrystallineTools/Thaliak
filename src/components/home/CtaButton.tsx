import { IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import Link from '../shared/Link';

export default function CtaButton({ faIcon, text, href }: { faIcon?: IconDefinition, text: string, href: string }) {
  const style = 'rounded-full bg-brand hover:bg-brand-bg active:bg-slate-900 px-5 py-3 whitespace-nowrap mr-2 mb-2 text-gray-100 text-lg font-light';

  const icon = faIcon && <FontAwesomeIcon icon={faIcon} className='mr-3' />;

  return (
    <Link href={href} className={style}>
      {icon}
      {text}
    </Link>
  );
}
