import { faCircleDollarToSlot, faCodePullRequest, faHome } from '@fortawesome/free-solid-svg-icons';
import SideNavigationItem from '../../shared/navigation/side/SideNavigationItem';
import SideNavigation from '../../shared/navigation/side/SideNavigation';
import iconGraphQL from './icon-graphql.svg';
import { donateLinkThaliak, githubLinkThaliak } from '../../../constants';

export default function ThaliakSideNavigation() {
  return (
    <SideNavigation>
      <SideNavigationItem
        faIcon={faHome}
        text='Repositories'
        href='/thaliak'
      />
      <SideNavigationItem
        text='GraphQL API'
        href='https://thaliak.xiv.dev/graphql/'
        customIcon={(<img
          alt=''
          aria-hidden='true'
          src={iconGraphQL}
          className='inline-block align-top'
        />)}
      />
      <SideNavigationItem
        faIcon={faCodePullRequest}
        text='Contribute'
        href={githubLinkThaliak}
      />
      <SideNavigationItem
        faIcon={faCircleDollarToSlot}
        text='Donate'
        href={donateLinkThaliak}
      />
    </SideNavigation>
  );
}
