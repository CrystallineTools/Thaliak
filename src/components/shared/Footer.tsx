import poweredByVercel from './powered-by-vercel.svg';

export default function Footer() {
  return (
    <div className='mt-3 container mx-auto text-gray-700 text-sm'>
      <div className='flex flex-wrap flex-col sm:flex-row'>
        <div className='w-4/5 mr-auto'>
          FINAL FANTASY is a registered trademark of Square Enix Holdings Co., Ltd.
          <br />
          FINAL FANTASY XIV © 2010-2022 SQUARE ENIX CO., LTD. All Rights Reserved.
          <br />
          The Ewer is not affiliated with SQUARE ENIX CO., LTD. in any way.
        </div>
        <div className='mt-2 sm:mt-0 relative flex-grow max-w-full flex-1 text-end flex flex-col'>
          <span>
            version{' '}
            <a className='underline'
               href={`https://github.com/EwerXIV/ewer-web/commit/${process.env.REACT_APP_GIT_SHA!}`}>
              {process.env.REACT_APP_GIT_SHA!}
              </a>
          </span>
          <a href='https://vercel.com/?utm_source=EwerXIV&utm_campaign=oss'>
            <img src={poweredByVercel} alt='Powered by Vercel' className='block mt-0.5 h-7 ml-auto' />
          </a>
        </div>
      </div>
    </div>
  );
}