# University Magazine Contribution System (UMCS)

The University **Magazine Contribution System (UMCS)** is a web-based platform designed to facilitate the submission and management of content for university magazines. Whether you’re a student, faculty member, or contributor, UMCS streamlines the process of sharing articles, essays, and other written pieces within the university community.

## Features
- User Authentication:
	- Secure login and registration for contributors.
	- Role-based access control (contributors, editors, administrators).
- Article Submission:
	- Contributors can submit their written content.
	- Rich text editor for formatting articles.
- Editorial Workflow:
	- Editors review and approve submitted articles.
	- Workflow status tracking (submitted, under review, published).
- Version Control:
	- Keep track of revisions and updates to articles.
	- Maintain historical versions for reference.
- Categorization:
	- Organize content by topics, departments, or themes.
	- Tagging system for easy content discovery.
 - Search and Retrieval:
	- Search functionality to find specific articles.
	- Filter by author, category, or publication date.
 ## Description:
 
|                                            Role                                          | Description                                                                                                        |
|:----------------------------------------------------------------------------------------:|------------------------------------------------------------------------------------------------------------------- |
|                                        Administrator                                     | The ones who maintain the system                                                                                   |
|                                      Marketing Manager                                   | The ones who view and download contribution                                                                        |
|                                    Marketing Coordinator                                 | The ones who will recive email when student submit contribution and need to comment the contribution within 14 days|
|                                          Student                                         | The ones who can submit their contribution and recive Coordinator comment                                          |
|                                           Guest                                          | The ones who can view the contribution                                                                             |

## Setup
### Database
- Create new database name UMCS
- Open UMCS.sql in database directory
- Exceute the script
### Config VS
- Run COMP1640.sln
- Open UmcsContext.cs
- Change the Data Source into your own
#### Notice
- If you not know your data source or add database into VS, pls watch this video: https://youtu.be/7SgVx1owKJQ?si=bKynmyGA2giVVp-r


## Account in database
|                                            Role                                          |                                       Account                                         |      Password       |
|:----------------------------------------------------------------------------------------:|:-------------------------------------------------------------------------------------:|:-------------------:|
|                                        Administrator                                     | administrator@gmail.com                                                               |          1          |
|                                      Marketing Manager                                   | marketingmanager@gmail.com                                                            |          1          |
|                                    Marketing Coordinator                                 | computingcoordinator@gmail.com                                                        |          1          |
|                                                                                          | businesscoordinator@gmail.com                                                         |          1          |
|                                                                                          | digitalmarketingcoordinator@gmaill.com                                                |          1          |
|                                          Student                                         | computingstudent@gmail.com                                                            |          1          |
|                                                                                          | businessstudent@gmail.com                                                             |          1          |
|                                                                                          | digitalmarketingstudent@gmaill.com                                                    |          1          |
|                                           Guest                                          | computingguest@gmail.com                                                              |          1          |
|                                                                                          | businessguest@gmail.com                                                               |          1          |
|                                                                                          | digitalmarketingguest@gmaill.com                                                      |          1          |

#### Notice : 
- If 1 is incorect you can try 2 insted
