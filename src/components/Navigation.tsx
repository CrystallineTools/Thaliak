import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faGithub } from '@fortawesome/free-brands-svg-icons';
import { Container, Nav, Navbar } from 'react-bootstrap';

export default function Navigation() {
  return (
    <Navbar bg='dark' variant='dark' className='mb-3'>
      <Container>
        <Navbar.Brand href='/'>
          <img
            alt='logo'
            src='/logo.svg'
            width='30'
            height='30'
            className='d-inline-block align-top'
          />{' '}
          Thaliak</Navbar.Brand>
        <Nav className='me-auto'>
          <Nav.Link href='/'>Home</Nav.Link>
          <Nav.Link href='/api/'>API Docs</Nav.Link>
        </Nav>
        <Nav className='ms-auto'>
          <Nav.Link href='https://github.com/avafloww/Thaliak'>
            <FontAwesomeIcon icon={faGithub} />
          </Nav.Link>
        </Nav>
      </Container>
    </Navbar>
  );
}
