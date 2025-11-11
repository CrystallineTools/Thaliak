export default function Footer() {
  return (
    <footer className='mt-12 py-8 border-t border-gray-200'>
      <div className='container mx-auto'>
        <div className='flex flex-wrap flex-col sm:flex-row gap-4 items-start sm:items-center justify-between text-sm text-gray-600'>
          <div className='grow'>
            <p className='mb-1'>FINAL FANTASY is a registered trademark of Square Enix Holdings Co., Ltd.</p>
            <p className='mb-1'>FINAL FANTASY XIV © 2010-2025 SQUARE ENIX CO., LTD. All Rights Reserved.</p>
            <p>We are not affiliated with SQUARE ENIX CO., LTD. in any way.</p>
          </div>
          {import.meta.env.VITE_GIT_SHA && (
            <div className='flex items-center gap-2 bg-gray-100 px-3 py-2 rounded-lg'>
              <svg className='w-4 h-4 text-gray-500' fill='currentColor' viewBox='0 0 20 20'>
                <path fillRule='evenodd' d='M10 18a8 8 0 100-16 8 8 0 000 16zm1-11a1 1 0 10-2 0v2H7a1 1 0 100 2h2v2a1 1 0 102 0v-2h2a1 1 0 100-2h-2V7z' clipRule='evenodd' />
              </svg>
              <span className='text-gray-600'>
                version{' '}
                <a
                  className='font-mono text-primary-600 hover:text-primary-700 font-medium'
                  href={`https://github.com/CrystallineTools/Thaliak/commit/${import.meta.env.VITE_GIT_SHA}`}
                  target='_blank'
                  rel='noopener noreferrer'>
                  {import.meta.env.VITE_GIT_SHA}
                </a>
              </span>
            </div>
          )}
        </div>
      </div>
    </footer>
  );
}
