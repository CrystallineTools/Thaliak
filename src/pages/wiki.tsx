import Loading from '../components/shared/Loading';
import { useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { Wiki } from '../api/wiki';
import { marked } from 'marked';
import * as DOMPurify from 'dompurify';
import hljs from 'highlight.js/lib/common';
import 'highlight.js/styles/github.css';
import './wiki.css';

import MarkedOptions = marked.MarkedOptions;

interface PageMeta {
  title: string;
  description: string;
}

interface Page {
  meta: PageMeta;
  text: string;
}

const MARKED_OPTIONS: MarkedOptions = {
  gfm: true,
  xhtml: true,
  baseUrl: '/wiki',
  highlight: (code, lang) => {
    if (lang) {
      return hljs.highlight(code, { language: lang, ignoreIllegals: true }).value;
    } else {
      return hljs.highlightAuto(code).value;
    }
  }
};

const DOMPURIFY_OPTIONS: DOMPurify.Config = {
  USE_PROFILES: { html: true }
};

export function RootWikiPage() {
  const path = useParams()['*'] ?? 'index';
  return (
    <WikiPage path={path} />
  );
}

export function WikiPage({ path, hideHeader }: { path: string, hideHeader?: boolean }) {
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState<Page | null>(null);

  useEffect(() => {
    Wiki.get(path).then(setPage).finally(() => setLoading(false));
  }, [path]);

  if (loading) {
    return <Loading />;
  }

  const unsafeMarkdown = marked.parse(page?.text ?? 'Page not found.', MARKED_OPTIONS);

  const title = DOMPurify.sanitize(page?.meta.title ?? '', DOMPURIFY_OPTIONS);
  const description = DOMPurify.sanitize(page?.meta.description ?? '', DOMPURIFY_OPTIONS);
  const content: string = DOMPurify.sanitize(unsafeMarkdown, DOMPURIFY_OPTIONS) as string;

  return (
    <>
      {!hideHeader && (
        <div className='mb-4'>
          <div className='text-3xl font-bold'>
            {title}
          </div>
          <div className='text-gray-500 text-sm'>
            {description}
          </div>
        </div>
      )}
      <div className='wiki-page' dangerouslySetInnerHTML={{ __html: content }} />
    </>
  );
}
