export default function Footer() {
  return (
    <div className='mt-3 container mx-auto text-gray-700 text-sm'>
      <div className='flex flex-wrap flex-col sm:flex-row'>
        <div className='w-4/5 mr-auto'>
          FINAL FANTASY is a registered trademark of Square Enix Holdings Co., Ltd.
          <br />
          FINAL FANTASY XIV © 2010-2022 SQUARE ENIX CO., LTD. All Rights Reserved.
          <br />
          We are not affiliated with SQUARE ENIX CO., LTD. in any way.
        </div>
        <div className='mt-2 sm:mt-0 relative flex-grow max-w-full flex-1 text-end'>
          version{' '}
          <a className='underline'
             href={`https://github.com/avafloww/thaliak-web/commit/${process.env.REACT_APP_GIT_SHA!}`}>
            {process.env.REACT_APP_GIT_SHA!}
          </a>
        </div>
      </div>
    </div>
  );
}