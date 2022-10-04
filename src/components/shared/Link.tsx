import { Link as InternalLink } from 'react-router-dom';
import { ReactNode } from 'react';

export default function Link({ href, children, ...props }: { href: string, children: ReactNode, [key: string]: any }) {
  if (href.match(/^https?:\/\//)) {
    return <a href={href} {...props}>{children}</a>;
  } else {
    return <InternalLink to={href} {...props}>{children}</InternalLink>;
  }
}
