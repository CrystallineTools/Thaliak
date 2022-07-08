export default function Footer() {
  return (
    <div className='mt-3 container text-muted small'>
      <div className='row'>
        <div className='col-10 me-auto'>
          FINAL FANTASY is a registered trademark of Square Enix Holdings Co., Ltd.
          <br />
          FINAL FANTASY XIV © 2010-2022 SQUARE ENIX CO., LTD. All Rights Reserved.
          <br />
          We are not affiliated with SQUARE ENIX CO., LTD. in any way.
        </div>
        <div className='col text-end'>
          version{' '}
          <a className='text-reset'
             href={`https://github.com/avafloww/thaliak-web/commit/${process.env.REACT_APP_GIT_SHA!}`}>
            {process.env.REACT_APP_GIT_SHA!}
          </a>
        </div>
      </div>

    </div>
  );
}