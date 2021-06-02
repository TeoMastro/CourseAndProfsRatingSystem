import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
export class NavMenu extends Component {
    static displayName = NavMenu.name;
    constructor(props) {
        super(props);
        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true,
        };
    }
    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    render() {
        return (
            <header className="head">
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white box-shadow" style={{padding: '0px'}} light>
                    <Container>
                        <NavbarBrand className="brandnav" style={{ color: 'white' }} tag={Link} to="/Main">CourseAndProfsClient</NavbarBrand>
                        <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                            <ul className="navbar-nav flex-grow">
                                {/*<NavItem>*/}
                                {/*    <NavLink><a className="menulink" href="https://login.it.teithe.gr/authorization/?client_id=60b80e0992f09141fabd0260&response_type=code&scope=profile&redirect_uri=https://courseandprofs.azurewebsites.net">Login</a></NavLink>*/}
                                {/*</NavItem>*/}
                            </ul>
                        </Collapse>
                    </Container>
                </Navbar>
            </header>
        );
    }
}