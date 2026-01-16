describe('Enterprise tests', () => {
    beforeEach(() => {
        cy.login();
    });
    it('Should load the header and display all navigation links', () => {
        cy.get('header nav.navbar').should('be.visible');

        cy.get('a.navbar-brand').should('have.attr', 'href', '/').and('contain', 'Motor pool');

        const links = [
            { text: 'Enterprises', href: '/' },
            { text: 'Reports', href: '/Reports' },
            { text: 'Brands', href: '/Brands' }
        ];

        links.forEach(link => {
            cy.get('nav ul.navbar-nav li a.nav-link')
                .contains(link.text)
                .should('have.attr', 'href', link.href)
                .and('have.class', 'text-dark');
        });

        cy.get('a#manage').should('contain', 'Hello admin!').and('have.attr', 'title', 'Manage');
        cy.get('button#logout').should('contain', 'Logout').and('have.class', 'nav-link');

        cy.get('form#logoutForm input[name="__RequestVerificationToken"]').should('have.attr', 'value');
    });

    it('Should toggle the navbar when the screen size is small', () => {
        cy.viewport('iphone-6');

        cy.get('button.navbar-toggler').should('be.visible');

        cy.get('button.navbar-toggler').click();
        cy.get('div.navbar-collapse').should('have.class', 'show');

        cy.get('button.navbar-toggler').click();
        cy.get('div.navbar-collapse').should('not.have.class', 'show');
    });

    it('Should display at least one enterprise with all properties set', () => {
        cy.get('.card').should('exist').and('have.length.greaterThan', 0);

        cy.get('.card').each(($card) => {
            cy.wrap($card).within(() => {
                cy.get('.card-title').should('not.be.empty');

                // Validate city, street, VAT, time zone, and managers info
                cy.get('.card-text').contains('City').should('not.be.empty');
                cy.get('.card-text').contains('Street').should('not.be.empty');
                cy.get('.card-text').contains('VAT').should('not.be.empty');
                cy.get('.card-text').contains('Time Zone').should('not.be.empty');
                cy.get('.card-text').contains('Managed by').should('contain.text', 'manager(s)');
                cy.get('.card-text').contains('Founded on').should('not.be.empty');

                // Validate the presence and correctness of links to Vehicles, Drivers, and Details
                cy.get('a[href*="Vehicles"]').should('have.attr', 'href').and('include', 'enterpriseId');
                cy.get('a[href*="Drivers"]').should('have.attr', 'href').and('include', 'enterpriseId');
            });
        });
    });
});