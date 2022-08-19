import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faDiscord, faGithub } from '@fortawesome/free-brands-svg-icons';
import { Badge, Container, Nav, Navbar, NavDropdown } from 'react-bootstrap';
import { faCircleDollarToSlot } from '@fortawesome/free-solid-svg-icons';
import { NavLink } from 'react-router-dom';
import { discordLink, githubMainRepoLink, githubSponsorsLink } from '../constants';

export default function Navigation() {
  return (
    <Navbar bg='dark' variant='dark' className='mb-3' expand='md'>
      <Container>
        <Navbar.Brand as={NavLink} to='/'>
          <img
            alt='logo'
            src='/logo.svg'
            width='30'
            height='30'
            className='d-inline-block align-top'
          />{' '}
          Thaliak
        </Navbar.Brand>
        <Navbar.Toggle aria-controls='main-nav' />
        <Navbar.Collapse id='main-nav' className='ms-auto'>
          <Nav className='me-auto'>
            <Nav.Link as={NavLink} to='/'>Repositories</Nav.Link>
            <NavDropdown
              title='API Docs'
              id='main-nav-dropdown-api-docs'>
              <NavDropdown.Item href='/graphql/'>
                <Badge pill bg='success'>
                  New
                </Badge>
                {' '}
                GraphQL API
              </NavDropdown.Item>
              <NavDropdown.Item href='/api/'>
                <Badge pill bg='danger'>
                  Deprecated
                </Badge>
                {' '}
                REST API
              </NavDropdown.Item>
            </NavDropdown>
          </Nav>
          <Nav className='ms-auto'>
            <Nav.Link href={discordLink}>
              <FontAwesomeIcon icon={faDiscord} />
            </Nav.Link>
            <Nav.Link href={githubSponsorsLink}>
              <FontAwesomeIcon icon={faCircleDollarToSlot} />
            </Nav.Link>
            <Nav.Link href={githubMainRepoLink}>
              <FontAwesomeIcon icon={faGithub} />
            </Nav.Link>
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}
